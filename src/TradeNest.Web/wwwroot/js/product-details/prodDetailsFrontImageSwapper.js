document.addEventListener('DOMContentLoaded', () => {
    const mainImgEl = document.querySelector('.product-main-image')
    const thumbnailImageEls = document.querySelectorAll('.product-thumb');
    
    thumbnailImageEls.forEach(ti => {
        ti.addEventListener("click", (e) => {
           mainImgEl.src = e.currentTarget.src;
        });
    });
});