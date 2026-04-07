document.addEventListener('change', function (event) {
    if (event.target.classList.contains('btn-check')) {
        const modal = event.target.closest('.modal');
        if (modal) {
            const submitButton = modal.querySelector('button[type="submit"]');
            const checkboxes = modal.querySelectorAll('.btn-check');
            const atLeastOneChecked = Array.from(checkboxes).some(cb => cb.checked);

            if (atLeastOneChecked) {
                submitButton.removeAttribute('disabled');
            } else {
                submitButton.setAttribute('disabled', 'disabled');
            }
        }
    }
});
