/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./**/*.{razor,html,cshtml}",
    "./wwwroot/**/*.{html,js}",
    "./Pages/**/*.{razor,cshtml}",
    "./Shared/**/*.{razor,cshtml}",
    "./Components/**/*.{razor,cshtml}"
  ],
  theme: {
    extend: {},
  },
  plugins: [],
}