using Framework.Core;
using Framework.Core.UnitOfWork;
using Framework.WebApi;
using Microsoft.AspNet.SignalR;
using Social.Application.AppServices;
using Social.Application.Dto;
using Social.Infrastructure;
using Social.WebApi.Hubs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Tweetinvi;
using Tweetinvi.Models;

namespace Social.WebApi.Controllers
{
    [RoutePrefix("api/twitter-accounts")]
    public class TwitterAccountsController : ApiController
    {
        private Lazy<IHubContext> _hubLazy = new Lazy<IHubContext>(() => GlobalHost.ConnectionManager.GetHubContext<TwitterHub>());

        private IHubContext _hub
        {
            get { return _hubLazy.Value; }
        }

        private IUnitOfWorkManager _uowManager;
        private ITwitterAccountAppService _appService;

        public TwitterAccountsController(
            IUnitOfWorkManager uowManager,
            ITwitterAccountAppService appService)
        {
            _uowManager = uowManager;
            _appService = appService;
        }

        /// <summary>
        /// Make a integration request, fron-end page should redirect to this api rather than calling this api directly.
        /// </summary>
        /// <param name="connectionId">singalr connection id.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("integration-request")]
        public IHttpActionResult IntegrationRequest([Required]string connectionId)
        {
            int siteId = _uowManager.Current.GetSiteId().Value;
            // Specify the url you want the user to be redirected to\
            string url = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + Url.Route("TwitterIntegrationCallback", new { siteId = Request.GetSiteId(), connectionId = connectionId });

            IAuthenticationContext authenticationContext = _appService.InitAuthentication(url);
            return Redirect(authenticationContext.AuthorizationURL);
        }

        /// <summary>
        /// This api is used by twitter.
        /// </summary>
        /// <param name="authorization_id"></param>
        /// <param name="connectionId">singalr connection id.</param>
        /// <param name="oauth_verifier"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("integration-callback", Name = "TwitterIntegrationCallback")]
        public async Task<IHttpActionResult> IntegrationCallback(string connectionId, string authorization_id, string oauth_verifier = null)
        {
            string errorInfo = string.Empty;
            try
            {
                await _appService.AddAccountAsync(authorization_id, oauth_verifier);
            }
            catch (ExceptionWithCode codeEx)
            {
                errorInfo = codeEx.Message;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            _hub.Clients.Client(connectionId).twitterAuthorize(errorInfo);
            return Ok();
        }

        /// <summary>
        /// Get accounts.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("accounts")]
        public IList<TwitterAccountListDto> GetAccounts()
        {
            return _appService.GetAccounts();
        }

        /// <summary>
        /// Get accounts and ignore permission check.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route()]
        public IList<TwitterAccountListDto> GetAccountsWithoutPermissionCheck()
        {
            return _appService.GetAccounts();
        }


        /// <summary>
        /// Get account by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("accounts/{id}")]
        [ResponseType(typeof(TwitterAccountDto))]
        public IHttpActionResult GetAccount(int id)
        {
            var page = _appService.GetAccount(id);
            if (page == null)
            {
                return StatusCode(HttpStatusCode.NotFound);
            }

            return Ok(page);
        }

        /// <summary>
        /// update account.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("accounts/{id}")]
        public TwitterAccountDto UpdateAccount(int id, [Required]UpdateTwitterAccountDto dto)
        {
            var account = _appService.UpdateAccount(id, dto);
            return account;
        }

        /// <summary>
        /// mark account as enabled or disabled.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ifEnable"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("accounts/{id}/if-enable")]
        public TwitterAccountDto MarkAsEnable(int id, bool? ifEnable = true)
        {
            var page = _appService.MarkAsEnable(id, ifEnable);
            return page;
        }

        /// <summary>
        ///  delete account.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("accounts/{id}")]
        public async Task<int> DeleteAccount(int id)
        {
            await _appService.DeleteAccountAsync(id);
            return id;
        }
    }
}