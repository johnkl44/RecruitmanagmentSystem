document.addEventListener('DOMContentLoaded', function () {
    const passwordInput = document.getElementById("Password");
    const confirmPasswordInput = document.getElementById("ConfirmPassword");
    const emailInput = document.getElementById("Email");
    const phoneInput = document.getElementById("PhoneNumber");
    const firstNameInput = document.getElementById("FirstName");
    const lastNameInput = document.getElementById("LastName");
 

    function checkJobTitle() {

        const jobTitleValue = jobTitleInput.value.trim();
        const jobTitleRegex = /^[A-Z][a-zA-Z\s]{2,}$/;

        if (jobTitleValue === "") {
            setError(jobTitleInput, "Job Title is required.");
        }
        else if (!jobTitleRegex.test(jobTitleValue)) {
            setError(
                jobTitleInput,
                "Provide a valid job title. The first letter must be capitalized."
            );
        }
        else {
            setSuccess(jobTitleInput);
            return true;
        }
    }
    function checkJobDescription() {
        const jobDescriptionValue = jobDescriptionInput.value.trim();
        const jobDescriptionRegex = /^[A-Z][a-zA-Z]{2,}$/;

        if (firstNameValue === "") {
            setError(firstNameInput, "First name is required.");
        }
        else if (!jobDescriptionRegex.test(jobDescriptionValue)) {
            setError(
                jobDescriptionInput,
                "provid valid name, first letter should be capital letter"
            );
        }
        else {
            setSuccess(jobDescriptionInput);
            return true;
        }
    }

    function checkFirstName() {
        const firstNameValue = firstNameInput.value.trim();
        const firstNameRegex = /^[A-Z][a-zA-Z]{2,}$/;

        if (firstNameValue === "") {
            setError(firstNameInput, "First name is required.");
        }
        else if (!firstNameRegex.test(firstNameValue)) {
            setError(
                firstNameInput,
                "provid valid name, first letter should be capital letter"
            );
        }
        else {
            setSuccess(firstNameInput);
            return true;
        }
    }

    function checkLastName() {
        const LastNameValue = lastNameInput.value.trim();
        const LastNameRegex = /^[A-Z][a-zA-Z]{2,}$/;

        if (LastNameValue === "") {
            setError(lastNameInput, "Last name is required.");
        }
        else if (!LastNameRegex.test(LastNameValue)) {
            setError(
                lastNameInput,
                "provid valid name, first letter should be capital letter"
            );
        }
        else {
            setSuccess(lastNameInput);
            return true;
        }
    }
    function checkConfirmPassword() {
        clearMessages(confirmPasswordInput); 
        const passwordValue = passwordInput.value.trim();
        const confirmPasswordValue = confirmPasswordInput.value.trim();

        if (confirmPasswordValue === "") {
            setError(confirmPasswordInput, "Please confirm your password.");
        } else if (confirmPasswordValue !== passwordValue) {
            setError(confirmPasswordInput, "Passwords do not match.");
        } else {
            setSuccess(confirmPasswordInput);
        }
    }

    function checkEmail() {
        clearMessages(emailInput);
        const emailValue = emailInput.value.trim();
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (emailValue === "") {
            setError(emailInput, "Email is required.");
        } else if (!emailRegex.test(emailValue)) {
            setError(emailInput, "Enter a valid email address.");
        } else {
            setSuccess(emailInput);
        }
    }

    function checkPassword() {
        const passwordInput = document.getElementById("Password");
        const passwordValue = passwordInput.value.trim();
        //Expression check Password must be at least 8 characters long, include one uppercase letter, one number, and one special character.
        const strongPasswordRegex = /^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$/;

        if (passwordValue === "") {
            setError(passwordInput, "Password is required.");
        }
        else if (!strongPasswordRegex.test(passwordValue))
        {
            setError(
                passwordInput,
                "Password must be at least 8 characters long, include one uppercase letter, one number, and one special character."
            );
        }
        else
        {
            setSuccess(passwordInput);
            return true;
        }
    }
    function checkPhoneNumber() {
        const phoneValue = phoneInput.value.trim();
        const phoneRegex = /^[0-9]{8,12}$/;

        if (phoneValue === "") {
            setError(phoneInput, "Phone number is required.");
        }
        else if (!phoneRegex.test(phoneValue)) {
            setError(
                phoneInput,
                "Valid phone number required"
            );
        }
        else {
            setSuccess(phoneInput);
            return true;
        }
    }

 


    function setError(input, message) {
        clearMessages(input); 
        const errorMessage = document.createElement('span');
        errorMessage.className = 'text-danger';
        errorMessage.innerText = message;

        input.classList.add('is-invalid');
        input.parentNode.appendChild(errorMessage);
    }

    function setSuccess(input) {
        input.classList.remove('is-invalid');
        clearMessages(input);
    }

    function clearMessages(input) {
        const errorMessages = input.parentNode.querySelectorAll('.text-danger');
        errorMessages.forEach(msg => msg.remove());
    }

    confirmPasswordInput.addEventListener("focusout", checkConfirmPassword);
    emailInput.addEventListener("focusout", checkEmail);
    passwordInput.addEventListener("focusout", checkPassword);
    phoneInput.addEventListener("focusout", checkPhoneNumber);
    firstNameInput.addEventListener("focusout", checkFirstName);
    lastNameInput.addEventListener("focusout", checkLastName);
    jobTitleInput.addEventListener("focusout", checkJobTitle);
    jobDescriptionInput.addEventListener("focusout", checkJobDescription);

    //Minor Date Picking
    var todayDate = new Date();
    var month = todayDate.getMonth() + 1;
    var year = todayDate.getFullYear() - 18;
    var tDate = todayDate.getDate();

    if (month < 10) {
        month = '0' + month;
    }
    if (tDate < 10) {
        tDate = '0' + tDate;
    }

    var maxDate = year + '-' + month + '-' + tDate;

    document.getElementById("DateOfBirth").setAttribute("max", maxDate);
});


  function checkJobTitle() {
            const jobTitleInput = document.getElementById("JobTitle");
            const jobTitleValue = jobTitleInput.value.trim();
            const jobTitleRegex = /^[A-Z][a-zA-Z\s'-]{2,}$/;

            if (jobTitleValue === "") {
                setError(jobTitleInput, "Job Title is required.");
            } else if (!jobTitleRegex.test(jobTitleValue)) {
                setError(jobTitleInput, "Provide a valid job title. The first letter must be capitalized.");
            } else {
                setSuccess(jobTitleInput);
            }
        }

        function checkJobDescription() {
            const jobDescriptionInput = document.getElementById("JobDescription");
            const jobDescriptionValue = jobDescriptionInput.value.trim();
            if (jobDescriptionValue === "") {
                setError(jobDescriptionInput, "Job Description is required.");
            } else {
                setSuccess(jobDescriptionInput);
            }
        }

        function checkRequiredSkills() {
            const requiredSkillsInput = document.getElementById("RequiredSkills");
            const requiredSkillsValue = requiredSkillsInput.value.trim();
            if (requiredSkillsValue === "") {
                setError(requiredSkillsInput, "Required Skills are required.");
            } else {
                setSuccess(requiredSkillsInput);
            }
        }

        function checkExperience() {
            const experienceInput = document.getElementById("Experience");
            const experienceValue = experienceInput.value.trim();
            if (experienceValue === "") {
                setError(experienceInput, "Experience is required.");
            } else {
                setSuccess(experienceInput);
            }
        }

        function checkSalaryRange() {
            const salaryRangeInput = document.getElementById("SalaryRange");
            const salaryRangeValue = salaryRangeInput.value.trim();
            if (salaryRangeValue === "") {
                setError(salaryRangeInput, "Salary Range is required.");
            } else {
                setSuccess(salaryRangeInput);
            }
        }

        function checkDeadline() {
            const deadlineInput = document.getElementById("Deadline");
            const deadlineValue = deadlineInput.value.trim();
            if (deadlineValue === "") {
                setError(deadlineInput, "Deadline is required.");
            } else {
                setSuccess(deadlineInput);
            }
        }

        function checkPosterPhoto() {
            const posterPhotoInput = document.getElementById("PosterPhoto");
            if (!posterPhotoInput.files.length) {
                setError(posterPhotoInput, "Please upload a poster photo.");
            } else {
                setSuccess(posterPhotoInput);
            }
        }

function setError(input, message) {
    clearMessages(input);
    const errorMessage = document.createElement('span');
    errorMessage.className = 'text-danger';
    errorMessage.innerText = message;

    input.classList.add('is-invalid');
    input.parentNode.appendChild(errorMessage);
}

function setSuccess(input) {
    input.classList.remove('is-invalid');
    clearMessages(input);
}

function clearMessages(input) {
    const errorMessages = input.parentNode.querySelectorAll('.text-danger');
    errorMessages.forEach(msg => msg.remove());
}

        // Event Listeners for validation
        document.getElementById("JobTitle").addEventListener("focusout", checkJobTitle);
        document.getElementById("JobDescription").addEventListener("focusout", checkJobDescription);
        document.getElementById("RequiredSkills").addEventListener("focusout", checkRequiredSkills);
        document.getElementById("Experience").addEventListener("focusout", checkExperience);
        document.getElementById("SalaryRange").addEventListener("focusout", checkSalaryRange);
        document.getElementById("Deadline").addEventListener("focusout", checkDeadline);
        document.getElementById("PosterPhoto").addEventListener("focusout", checkPosterPhoto);



