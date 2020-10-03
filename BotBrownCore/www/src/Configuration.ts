import { ConfigurationStep } from './ConfigurationStep'

export class Configuration {

    steps: Array<ConfigurationStep>;
    currentlyActiveStep: number;

    constructor() {
        console.log("bla");
        this.currentlyActiveStep = -1;
        this.steps = new Array<ConfigurationStep>();
    }

    addStep = (step: ConfigurationStep) => {
        console.log("blubb" + this.currentlyActiveStep);
        this.steps.push(step);
        if (this.currentlyActiveStep == -1) {
            this.currentlyActiveStep = 0;
        }
    }

    moveToNextStep = () => {
        console.log(this.currentlyActiveStep);
        this.currentlyActiveStep = 200;
    }

    isCompleted = () => {
        return this.steps.every(x => x.isCompleted);
    }
}