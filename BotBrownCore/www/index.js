'use strict';

import Sidebar from './Sidebar.js'

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
            null, 
            e(Sidebar),
            e('div', { id: "main-area" })
        );
    }
}

ReactDOM.render(e(App), document.getElementById('root'));