export interface Config {
    baseApiUrl: string;
}

const config: Config = require('./config.json');
export default config;