const path = require('path');

let plugins = [];

module.exports = {
	resolve: {
		alias: {
			'@scripts': path.resolve(__dirname, 'Scripts'),
		},
	},
	plugins,
};
