const allManageRolesModals = document.querySelectorAll(".manage-roles-modal");
allManageRolesModals?.forEach(manageRolesModal => {
    manageRolesModal?.addEventListener("change", () => {
        const allActionButtons = manageRolesModal.querySelectorAll(".action-btn");
        
        let oneIsPressed = false;
        for (const actionButton of allActionButtons) {
            if(actionButton.checked){
                oneIsPressed = true;
                break;
            }
        }
        
        const saveChangesButton = manageRolesModal?.querySelector(".save-changes-btn");
        if(oneIsPressed)
            saveChangesButton?.removeAttribute('disabled');
        else
            saveChangesButton?.setAttribute('disabled', 'disabled');
    });
});
