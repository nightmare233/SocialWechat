import { Reducer } from 'redux';
import { AppThunkAction } from './';
import filterService from '../services/FilterService';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.
export interface FilterListItem {
    name: string;
    conversationNum: number;
    id: number;
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.

export interface RequestFilterListAction {
    type: "REQUEST_FILTER_LIST";
    isLoading: boolean;
}

export interface ReceiveFilterListAction {
    type: "RECEIVE_FILTER_LIST";
    filters: FilterListItem[];
    isLoading: boolean;
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = RequestFilterListAction | ReceiveFilterListAction;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {
    requestFilterList: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        filterService.getFilterList()
            .then(data => {
                dispatch({ type: 'RECEIVE_FILTER_LIST', filters: data, isLoading: true });
            })

        dispatch({ type: 'REQUEST_FILTER_LIST', isLoading: false });
    }
}

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
const initialState: FilterListItem[] = new Array<FilterListItem>();

export const reducer: Reducer<FilterListItem[]> = (state: FilterListItem[], action: KnownAction) => {
    switch (action.type) {
        case 'REQUEST_FILTER_LIST':
            return state
        case 'RECEIVE_FILTER_LIST':
            return action.filters
    }

    return state || initialState;
};