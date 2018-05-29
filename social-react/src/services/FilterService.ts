import * as axios from 'axios';
import config from '../config';
import { FilterListItem } from '../store/FilterList'

export class FilterService {
    getFilterList(): Promise<FilterListItem[]> {
        return new Promise<FilterListItem[]>((resolve, reject) => {
            axios.default.get(config.baseApiUrl + "/filters?siteId=10000")
                .then(res => {
                    var filters = res.data as Promise<FilterListItem[]>
                    resolve(filters);
                })
        })
    }
}

const filterService: FilterService = new FilterService();
export default filterService;
