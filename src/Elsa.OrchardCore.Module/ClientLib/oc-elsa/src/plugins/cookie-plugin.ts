import {EventBus, EventTypes, Plugin} from "@elsa-workflows/elsa-workflows-designer";
import {Service} from "typedi";
import {AxiosRequestConfig} from "axios";

@Service()
export class CookiePlugin implements Plugin {
    constructor(eventBus: EventBus) {
        eventBus.on(EventTypes.HttpClient.ConfigCreated, this.onHttpClientConfigCreated);
    }

    async initialize(): Promise<void> {
    }

    private onHttpClientConfigCreated = async (e) => {
        debugger;
        const axiosConfig = e.config as AxiosRequestConfig;
        axiosConfig.withCredentials = true;
    };
}