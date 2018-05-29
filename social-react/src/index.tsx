import * as React from 'react';
import * as ReactDOM from 'react-dom';
import registerServiceWorker from './registerServiceWorker';
import './index.css';
import { Provider } from 'react-redux';
import { createBrowserHistory } from 'history';
import configureStore from './configureStore';
import { ApplicationState } from './store';
import FilterList from './components/FilterList'

// Create browser history to use in the Redux store
const history = createBrowserHistory();

// Get the application-wide store instance, prepopulating with state from the server where available.
const initialState = (window as any).initialReduxState as ApplicationState;

const store = configureStore(history, initialState);

ReactDOM.render(
  <Provider store={store}>
    <FilterList />
  </Provider>,
  document.getElementById('root') as HTMLElement
);
registerServiceWorker();
