window.showDialog = (element) => {
    if (element && element.showModal) {
        element.showModal();
    }
};

window.hideDialog = (element) => {
    if (element && element.close) {
        element.close();
    }
};