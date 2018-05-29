import { createStore, applyMiddleware, compose, Store } from 'redux';
import thunk from 'redux-thunk';
import { ApplicationState, reducers } from './store';
import { History } from 'history';

export default function configureStore(history: History, initialState?: ApplicationState) {
    // Build middleware. These are functions that can process the actions before they reach the store.

    const createStoreWithMiddleware = compose(
        applyMiddleware(thunk),
    )(createStore);

    // Combine all reducers and instantiate the app-wide store instance
    const store = createStoreWithMiddleware(reducers, initialState) as Store<ApplicationState>;

    return store;
}

