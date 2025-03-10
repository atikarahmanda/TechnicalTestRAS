  // tailwind.config.js
  const preline = require('preline/plugin');
  module.exports = {
    content: ['./index.html', './src/**/*.{vue,js,ts,jsx,tsx}'],
    theme: {
      extend: {
        fontFamily: {
          poppins: ["Poppins", "sans-serif"]
        }
      },
    },
    plugins: [
      preline,
      require('daisyui'),
    ],
    
   }
