function changePassword(password) {
    return new Promise((resolve, reject) => {
        try {
            const formData = new FormData();
            formData.append('password', password);

            const requestOptions = {
                method: 'POST',
                body: formData
            };

            fetch(`/Account/UpdatePassword`, requestOptions)
                .then(response => {
                    if (response.ok) {
                        resolve();
                    } else {
                        throw new Error('Password Doesn\'t Changed!');
                    }
                })
                .catch(error => {
                    reject(error);
                });
        } catch (error) {
            reject(error);
        }
    });
}

function ChangePass() {
    try {
        Swal.fire({
            title: 'Change Password',
            html:
                '<input id="swal-input1" class="swal2-input rounded-4 w-75 py-2" type="password" placeholder="Enter new password">' +
                '<input id="swal-input2" class="swal2-input rounded-4 w-75 py-2" type="password" placeholder="Confirm new password">',
            preConfirm: () => {
                return new Promise((resolve, reject) => {
                    const password1 = document.getElementById('swal-input1').value;
                    const password2 = document.getElementById('swal-input2').value;
                    if (!password1 || !password2) {
                        Swal.showValidationMessage(`Please enter a password`);
                        reject('Please enter a password');
                    } else if (password1.length < 6 || password2.length < 6) {
                        Swal.showValidationMessage(`Password must be at least 6 characters long`);
                        reject('Password must be at least 6 characters long');
                    } else if (password1 !== password2) {
                        Swal.showValidationMessage(`The passwords do not match`);
                        reject('The passwords do not match');
                    } else {
                        resolve({ password1, password2 });
                    }
                });
            },
            customClass: {
                popup: "rounded-5",
                confirmButton: 'rounded-4 py-1 px-5 fs-5'
            },
            confirmButtonText: 'Change Password'
        }).then((result) => {
            if (result.isConfirmed) {
                const password = result.value.password1;
                changePassword(password)
                    .then(() => {
                        Swal.fire({
                            title: 'Success',
                            text: 'Password Changed',
                            icon: 'success',
                            customClass: {
                                popup: "rounded-5",
                                confirmButton: 'rounded-4 py-1 px-5 fs-5'
                            }
                        });
                    })
                    .catch((error) => {
                        Swal.fire('Error', error.message, 'error');
                    })
                    .finally(() => {
                        location.reload();
                    });
            }
        });
    } catch (error) {
        Swal.fire('Error', error.message, 'error');
    }
}

document.getElementsByClassName('nav-link ')[1].classList.add('active');