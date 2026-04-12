export function toggleHighlight(approvalStatus) {
    const label1 = document.querySelector('label[for="btnradio1"]');
    const label2 = document.querySelector('label[for="btnradio2"]');

    if(approvalStatus === "Approved") {
        label1.classList.replace("btn-outline-teal", "btn-teal");
        label2.classList.replace("btn-teal", "btn-outline-teal");
    } else {
        label2.classList.replace("btn-outline-teal", "btn-teal");
        label1.classList.replace("btn-teal", "btn-outline-teal");
    }
}
