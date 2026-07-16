document.addEventListener("DOMContentLoaded", () => {
    const searchFormEl = document.querySelector<HTMLFormElement>("#searchForm");

    searchFormEl?.addEventListener("submit", (e) => {
        const inputField = searchFormEl.querySelector<HTMLInputElement>("#searchInput")!;

        if(!inputField.value.trim()) {
            e.preventDefault();
        }
    });
});