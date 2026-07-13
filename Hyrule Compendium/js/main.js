const prefix = "cea6723";
const entryKey = prefix + "entry";

window.onload = (e) => {
    // assign method to each button
    let buttons = document.querySelectorAll("#buttons img");
    for (let button of buttons) {
        button.onclick = categoryButtonClicked;
    }
    // get last item clicked
    const storedEntry = JSON.parse(localStorage.getItem(entryKey));
    // display it
    if (storedEntry.length > 0) {
        let display = document.querySelector("#big-display");
        display.innerHTML = `
            <h2>${storedEntry[0]}</h2>
            <img src="${storedEntry[2]}" alt="${storedEntry[1]}">
            <p>${storedEntry[1]}</p>`;
    }
};

// formats the url and gets the coresponding data when the button is clicked
function categoryButtonClicked(e) {
    const categoryURL = "https://api.hyrule-compendium.com/v3/compendium/category/";
    let url = categoryURL + e.target.dataset.category;
    // console.log(url);
    getData(url);
    // scrolls back to the top when button is clicked
    document.querySelector("#content").scrollTop = 0;
}

// when a small image is clicked, display it in the bid-display section
function imageClicked(e) {
    let name = e.target.title;
    let description = e.target.alt;
    let url = e.target.src;

    // store the info
    let items = [name, description, url];
    localStorage.setItem(entryKey, JSON.stringify(items));

    // format the html that goes in the big-display
    let display = document.querySelector("#big-display");
    display.innerHTML = `
        <h2>${name}</h2>
        <img src="${url}" alt="${description}">
        <p>${description}</p>`;
}

// get data from the api
function getData(url) {
    // updated XML Request to fetch
    fetch(url)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(e => {
            renderCompendium(e);
        })
        .catch(error => {
            console.error('fetching error:', error);
        })
}

// Callback Functions
function renderCompendium(e) {
    let results = e.data;
    // see if you need to sort by name (default sorts by id)
    if (document.querySelector("#sort").value == "name") {
        results = results.sort((a, b) => a.name.localeCompare(b.name));
    }
    // console.log("results.length = " + results.length);
    let bitstring = "";
    // 10 - loop through the array of results
    for (let i = 0; i < results.length; i++) {
        let result = results[i];
        let imgURL = result.image;
        if (!imgURL) imgURL = "images/no-image-found.png";
        let name = result.name;
        let description = result.description;
        // build HTML
        let line = `<div class='result'>
        <img src="${imgURL}" alt= "${description}" title="${name}" />
        <p>${name}</p>
        </div>`
        bitstring += line;
    }
    // 16 - all done building the HTML - show it to the user!
    document.querySelector("#content").innerHTML = bitstring;

    // assign method to each image
    let images = document.querySelectorAll("#content img");
    for (let image of images) {
        image.onclick = imageClicked;
    }
}

// if data was not loaded correctly
function dataError(e) {
    console.log("An error occurred");
}