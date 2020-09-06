'use strict';

const e = React.createElement;

export default class MainArea extends React.Component {
    constructor(props) {
        super(props);
        this.state = { liked: false };
    }

    render() {
        return e('div', { id: "main-area" });
    }
}