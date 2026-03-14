document.addEventListener("DOMContentLoaded", () => {
    const searchFormEl = document.querySelector("#searchForm");
    
    searchFormEl?.addEventListener("submit", (e) => {
        const inputField = searchFormEl.querySelector("#searchInput");
        
        if(!inputField.value.trim()) {
            e.preventDefault();
        }
    });
});