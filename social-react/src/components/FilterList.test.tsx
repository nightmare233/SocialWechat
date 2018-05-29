import * as React from 'react';
import * as enzyme from 'enzyme';
import FilterList from './FilterList';

it('renders the filter list', () => {
    const filterList = enzyme.shallow(<FilterList />);
    expect(filterList != null);
});