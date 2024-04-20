const dropdownTriggers = document.querySelectorAll('.dropdown-trigger button');
dropdownTriggers.forEach((trigger) => {
    trigger.addEventListener('click', () => {
        trigger.parentElement.parentElement.classList.add('is-active');
    });
});

const addButtons = document.querySelectorAll('.add');
addButtons.forEach((button) => {
    button.addEventListener('click', () => {
        const modal = document.querySelector('.add-modal');
        modal.classList.add('is-active');
    });
});

const resetButtons = document.querySelectorAll('.reset');
resetButtons.forEach((button) => {
    button.addEventListener('click', () => {
        const inputElement = button.parentElement.querySelector('.input');
        inputElement.value = '';
    });
});

// listener du document pour enlever les class des div lorsqu'on click sur d'autres élements
document.addEventListener('click', (e) => {
    dropdownTriggers.forEach((trigger) => {
        if (!trigger.contains(e.target)) {
            trigger.parentElement.parentElement.classList.remove('is-active');
        }
    });
});

// Désactiver les skeletons et la pageLoader
document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.has-skeleton').forEach((skeleton) => {
        skeleton.classList.remove('has-skeleton');
    });
    document.querySelectorAll('.is-skeleton').forEach((skeleton) => {
        skeleton.classList.remove('is-skeleton');
    });
    document.querySelectorAll('.skeleton-block').forEach((skeleton) => {
        skeleton.classList.remove('skeleton-block');
    });
    document.querySelectorAll('.skeleton-lines').forEach((skeleton) => {
        skeleton.classList.remove('skeleton-lines');
    });
    document.querySelector('.pageloader').classList.remove('is-active');
});

// TODO la facon pour stocker la valeur du switch du dark mode
const switchMode = document.querySelector('#switch-mode');
switchMode.addEventListener('change', () => {
    if (localStorage.getItem('theme') === null) localStorage.setItem(document.documentElement.getAttribute('data-theme'));
    const newTheme = localStorage.getItem('theme') === 'light' ? 'dark' : 'light';
    document.documentElement.setAttribute('data-theme', newTheme);
    localStorage.setItem('theme', newTheme);
});

function deleteItem (link) {
    const itemId = link.getAttribute('data-id');
    const url = link.getAttribute('data-url');
    Ajax('DELETE', url + itemId)
        .then(response => {
            console.log('Élément supprimé avec succès');
            location.reload();
        })
        .catch(error => {
            console.error('Une erreur s\'est produite lors de la suppression:', error);
        });
}

async function getById(link) {
    const itemId = link.getAttribute('data-id');
    const url = link.getAttribute('data-url');
    return await Ajax('GET', url + itemId);
}

async function updateItem(link) {
    try {
        const response = await getById(link);
        const modal = document.querySelector('.update-modal');
        Object.keys(response).forEach(key => {
            const input = modal.querySelector(`[name="${key}"]`);
            if (input) {
                input.value = response[key];
            }
        });
        modal.classList.add('is-active');
    } catch (error) {
        console.error('Une erreur s\'est produite lors de la récupération de données:', error);
    }
}

async function searchItem(link) {
    const url = link.getAttribute('data-url');
    const data = { "search": link.value };
    const response = await Ajax('POST', url, data);
    console.log(response);
}