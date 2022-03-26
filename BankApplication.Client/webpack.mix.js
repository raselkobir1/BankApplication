let mix = require('laravel-mix');
const webpack = require("./webpack.config.js");


mix
.js('./Scripts/Application.js', './Output/Scripts').vue({ version: 2 })
.sass('./Styles/Application.scss', './Output/Styles')
.options({
    processCssUrls: false,
})
.sourceMaps()
.copyDirectory("./Assets/Images", "./Output/Assets/Images")
//.copyDirectory("./node_modules/font-awesome/fonts", "./Output/Assets/Fonts/FontAwesome")
.copyDirectory("./Output/scripts", "../BankApplication.Web/wwwroot/Scripts")
.copyDirectory("./Output/styles", "../BankApplication.Web/wwwroot/Styles")
.copyDirectory("./Output/Assets", "../BankApplication.Web/wwwroot/Assets")
.webpackConfig(Object.assign(webpack));
