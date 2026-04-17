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

export function showErrorSwal() {
    return Swal.fire({
        icon: "error",
        title: "Oops...",
        text: "Something went wrong! Please try again.",
        confirmButtonColor: "#0FAF9A",
        draggable: true,
        showClass: { popup: ` animate__animated animate__fadeInUp animate__faster ` },
        hideClass: { popup: ` animate__animated animate__fadeOutDown animate__faster ` }
    });
}

export function showPlainSuccessSwal(text) {
    return Swal.fire({
        icon: "success",
        title: "Success",
        text: text ?? "The operation has passed successfully.",
        draggable: true,
        confirmButtonColor: "#0FAF9A",
        showClass: { popup: ` animate__animated animate__fadeInUp animate__faster ` },
        hideClass: { popup: ` animate__animated animate__fadeOutDown animate__faster ` },
    })
}