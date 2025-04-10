const express = require('express');
const path = require('path'); // Require the path module
const { ref } = require('process');
const app = express();

const PORT = process.env.PORT || 3000;

app.use(express.static(path.join(__dirname, 'public')));

// Serve the HTML file for the root route
app.get('/', (req, res) => {
    res.sendFile(path.join(__dirname, 'index.html'));
});

app.get('/opensesame', (req, res) => {
    res.json({ message: "If you made it this far, you're awesome! But, if you can solve the riddle below, you win the golden egg....", riddle: "Before it became a product, Kakfa was a famous 20th literary author from Germany. Do you know his first name? It sounds similar to a country that we would love to greet first..." });
});

app.listen(PORT, () => {
  console.log(`Server running on port ${PORT}`);
});