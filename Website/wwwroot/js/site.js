// For Logout Alert 
function LogoutAlert(e) {
    e.preventDefault();
    Swal.fire({
        title: 'Are you sure to logout',
        icon: 'question',
        showCancelButton: true,
    }).then(result => {
        if (result.isConfirmed) {
            const logoutForm = document.getElementById('logoutForm');
            logoutForm.submit();
        }
    })
}

