'use strict';

const e = React.createElement;

class App extends React.Component {
    constructor(props) {
        super(props);
        this.state = { liked: false };
    }

    render() {
        if (this.state.liked) {
            return 'You bla liked this.';
        }

        return e(
            MaterialUI.List,
            {
                component: 'nav',
                'aria-label': 'main mailbox folders'
            },
            e(
                MaterialUI.ListItem,
                { button: 'true' },
                e(MaterialUI.ListItemIcon, null, 'Item 1')
                //e(MaterialUI.ListItemText, { primary: "Inbox" })
                //'Test1'
            ),
            e(
                MaterialUI.ListItem,
                { button: 'true' },
                e(MaterialUI.ListItemIcon, null, 'Item 2')
                //e(MaterialUI.ListItemText, { primary: "Inbox" })
                //'Test1'
            ),
            e(
                MaterialUI.ListItem,
                { button: 'true' },
                e(MaterialUI.ListItemIcon, null, 'Item 3')
                //e(MaterialUI.ListItemText, { primary: "Inbox" })
                //'Test1'
            )
        );
    }
}

ReactDOM.render(e(App), document.getElementById('root'));