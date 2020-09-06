'use strict';

import Sidebar from './Sidebar.js';
import MainArea from './MainArea.js';

const e = React.createElement;

export class App extends React.Component {
    constructor(props) {
        super(props);
        this.state = { liked: false };
    }

    render() {
        if (this.state.liked) {
            return 'You bla liked this.';
        }

        return e(
            'div',
            {
                id: "app"

            }, 
            e(Sidebar),
            e(MainArea),
        );
    }
}

ReactDOM.render(e(App), document.getElementById('root'));