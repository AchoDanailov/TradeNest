const radio1 = document.querySelector("#btnradio1");
const label1 = document.querySelector('label[for="btnradio1"]');

const radio2 = document.querySelector("#btnradio2");
const label2 = document.querySelector('label[for="btnradio2"]');

radio1.addEventListener("change", () => {
    label1.classList.replace("btn-outline-teal", "btn-teal");
    label2.classList.replace("btn-teal", "btn-outline-teal");
});

radio2.addEventListener("change", () => {
    label2.classList.replace("btn-outline-teal", "btn-teal");
    label1.classList.replace("btn-teal", "btn-outline-teal");
});
