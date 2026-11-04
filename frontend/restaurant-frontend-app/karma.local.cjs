// Unit-test server is reachable only from this computer, not the home/LAN network.
module.exports = function (config) {
  config.set({
    basePath: '',
    frameworks: ['jasmine'],
    plugins: [require('karma-jasmine'), require('karma-chrome-launcher')],
    reporters: ['progress'],
    hostname: '127.0.0.1',
    listenAddress: '127.0.0.1',
    port: 9876,
    browsers: ['ChromeHeadlessLocal'],
    customLaunchers: {
      ChromeHeadlessLocal: { base: 'ChromeHeadless', flags: ['--no-proxy-server'] },
    },
    captureTimeout: 30000,
    browserNoActivityTimeout: 30000,
    singleRun: true,
    restartOnFileChange: false,
  });
};
