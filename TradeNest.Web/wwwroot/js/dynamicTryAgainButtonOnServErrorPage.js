const tryAgainButtonEl = document.createElement('a');

tryAgainButtonEl.textContent = "Try Again";

tryAgainButtonEl.classList.add("btn");
tryAgainButtonEl.classList.add("btn-outline-teal");
tryAgainButtonEl.classList.add("btn-lg");
tryAgainButtonEl.classList.add("px-4");
tryAgainButtonEl.classList.add("shadow-sm");

tryAgainButtonEl.addEventListener("click", async () => {
   await window.history.back(); 
});

const buttonContainerEl = document.querySelector("#btn-container");
buttonContainerEl.appendChild(tryAgainButtonEl);