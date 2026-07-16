document.addEventListener('DOMContentLoaded', () => {
    const mainImgEl = document.querySelector<HTMLImageElement>('.product-main-image')!
    const thumbnailImageEls = document.querySelectorAll<HTMLImageElement>('.product-thumb')!;

    thumbnailImageEls.forEach(ti => {
        ti.addEventListener("click", (e) => {
           mainImgEl.src = (e.currentTarget as HTMLImageElement).src;
        });
    });
});