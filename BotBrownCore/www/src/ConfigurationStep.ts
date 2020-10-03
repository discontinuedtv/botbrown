export class ConfigurationStep {

    isCompleted: boolean;
    content: any;

    constructor() {
        this.isCompleted = false;
    }

    setContent(content: any) {
        this.content = content;
    }
}