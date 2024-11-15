function loadFilteredMenuItem(categoryName) {
    fetch(`/Admin/MenuItems?categoryName=${categoryName}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Lỗi khi tải danh sách món ăn theo danh mục');
            }
            return response.text();
        })
        .then(html => {
            document.getElementById('menuItemsTable').innerHTML = html; // Cập nhật nội dung bảng
        })
        .catch(error => {
            console.error(error);
        });
}

document.addEventListener('click', function (e) {
    if (e.target.matches('.pagination a')) {
        e.preventDefault();
        const url = e.target.getAttribute('href');
        const container = e.target.closest('#menuitem-container') ? '#menuitem-container' : '#staff-container';
        fetchContent(url, container);
    }
});