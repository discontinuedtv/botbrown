'use strict';

const e = React.createElement;

export default class Sidebar extends React.Component {
    constructor(props) {
        super(props);
        this.state = { liked: false };
    }

    render() {
        return e(
            MaterialUI.List,
            {
                disablePadding: "",
                dense: ""
            },
            e(
                MaterialUI.ListItem,
                { button: 'true' },
                e(MaterialUI.ListItemText, null, 'Home')
            ),
            e(
                MaterialUI.ListItem,
                { button: 'true' },
                e(MaterialUI.ListItemText, null, 'Test 1')
            ),
            e(
                MaterialUI.ListItem,
                { button: 'true' },
                e(MaterialUI.ListItemText, null, 'Test 2')
            )
        );
    }
}