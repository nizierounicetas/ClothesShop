function initDisabledInputAmountAll() {
    const table = document.querySelector('form table');
    const checkboxes = table.querySelectorAll('input[type="checkbox"]');

    checkboxes.forEach((checkbox) => {
        initDisabledInputAmount(checkbox.id);
    });
}

function initDisabledInputAmount(id) {
    const table = document.querySelector('form table');
    checkbox = table.querySelector('input[type="checkbox"][id="' + id + '"]');
    inputAmount = table.querySelector('input[type="number"][id="' + id + '"]');
    spanValidation = table.querySelector('span.error-validation[id="' + id + '"]');

    inputAmount.disabled = !checkbox.checked;
    if (!checkbox.checked) {
        inputAmount.value = 0;
        spanValidation.textContent = '';

    }
    else {
        document.getElementById("size-validation").innerText = '';
    }
}

function initPostprocessingValidationSpans() {
    const table = document.querySelector('form table');
    const spans = table.querySelectorAll('span.error-validation');

    spans.forEach((span) => {
        span.addEventListener('DOMSubtreeModified', function () {
            input = document.querySelector('input[type="number"][id="' + span.id + '"]');
            if (input.value >= 0) {
                span.textContent = '';
            }
        });
    });
}

//window.isImageValid = true;

function previewImage(event) {
    let image = document.getElementById("preview-image");
    let files = document.getElementById("upload-image").files;
    document.getElementById("image-validation").innerText = '';
    if (files.length > 0) {
        let fileReader = new FileReader();

        fileReader.onload = function (e) {
            image.src = e.target.result;

            imgTest = new Image();
            imgTest.onerror = function () {
                document.getElementById("upload-image").value = null;
                document.getElementById("image-validation").innerText = 'Wrong image';
                image.src = "/assets/no-photos.png";
              //  window.isImageValid = false;
            };

            imgTest.onload = function () {
                window.isImageValid = true;
            }

            imgTest.src = e.target.result;
        };


        fileReader.readAsDataURL(files[0]);
    }
    else {
        document.getElementById("preview-image").removeAttribute("src");
        image.src = "/assets/no-photos.png";
     //   window.isImageValid = true;
    }
}

function validateSizeChosen() {
    const form = document.querySelector('form');
    const checkboxes = form.querySelectorAll('input[type="checkbox"]');

    if (checkboxes != null) 
    {
        for (let i = 0; i < checkboxes.length; i++) {
            if (checkboxes[i].checked) {
                document.getElementById("size-validation").innerText = '';
                return true;
            }
        }
    }

    document.getElementById("size-validation").innerText = 'No size chosen';
    return false;
}

function removeInitialImage(event) {
    checkbox = document.getElementById("remove-image");
    checkbox.checked = true;

    image = document.getElementById("preview-image");
    image.src = "/assets/no-photos.png";

    event.target.remove();
}


async function validateImage(event) {

    if (!window.isImageValid) {
            console.log('kjgnvf')
            document.getElementById("image-validation").innerText = 'Wrong image';
        return false;
        }
    /*console.log('here12');
    event.preventDefault();

    let form = document.getElementById("create-form");
    let input = document.getElementById("upload-image");

    if (input.files.length == 0) {
        form.submit();
        return;
    }

   let reader = new FileReader();
    reader.onload = function (e) {

        let img = new Image();

        img.onerror = function () {
            document.getElementById("image-validation").innerText = 'Wrong image';
        };

        img.onload = function (e) {
            form.submit();
        }

        img.src = e.target.result;
    };

    reader.onloadend = () => {
        console.log('end');
    }

    reader.readAsDataURL(input.files[0]);
    return;*/
}
