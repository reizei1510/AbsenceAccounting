// для обновления таблицы после изменения записи
import { createTable } from './records.js';

// переменная для хранения текущих данных формы
let initialData;

// функция заполнения выпадающего списка сотрудниками
async function fillEmployees() {
    // получаем список сотрудников
    const response = await fetch(`/api/employees/`, {
        method: `GET`,
        headers: {
            "Accept": "application/json"
        }
    });
    if (response.ok) {
        const employeesList = await response.json();

        const employeesSelect = document.querySelector(`#employee`);
        employeesSelect.innerHTML = ``;

        employeesList.forEach((employee) => {
            const employeeOption = document.createElement(`option`);
            employeeOption.value = employee.split(' ')[0];
            employeeOption.textContent = employee;

            employeesSelect.append(employeeOption);
        });
    }
    else {
        alert(`Не удалось загрузить список сотрудников`);
    }
}

// функция вызова формы для добавления/изменения записи
export async function openForm(id) {
    // добавление новой записи
    if (id == 0) {
        document.querySelector(`#save-record-method`).value = `POST`;
        document.querySelector(`#form-title`).innerHTML = `Добавление записи об отсутствии`;

        // получаем id будущей записи
        const response = await fetch(`/api/records/next-id/`, {
            method: `GET`,
            headers: {
                "Accept": `application/json`
            }
        });
        if (response.ok) {
            const id = await response.json();
            document.querySelector(`#recordId`).value = id;
        }
        else {
            alert('Не удалось получить идентификатор записи');
        }
    }
    // изменение записи по id
    else {
        document.querySelector(`#form-title`).innerHTML = `Изменение записи об отсутствии`;

        // получаем данные записи
        const response = await fetch(`/api/records/${id}`, {
            method: `GET`,
            headers: {
                "Accept": `application/json`
            }
        });
        if (response.ok) {
            const recordData = await response.json();
            
            document.querySelector(`#save-record-method`).value = `PUT`;

            // добавление значений в форму
            for (const key in recordData) {
                if (recordData.hasOwnProperty(key)) {
                    const form = document.querySelector(`#record-form`);
                    const element = form.elements[key];
                    if (element) {
                        if (element.type == `checkbox`) {
                            element.checked = recordData[key];
                        }
                        else if (element.type == `select-one`) {
                            element.selectedIndex = recordData[key].split(` `)[0] - 1;
                        }
                        else {
                            element.value = recordData[key];
                        }
                    }
                }
            }
        }
        else {
            alert(`Не удалось загрузить данные о записи`);
            return;
        }
    }

    // отображаем форму
    document.querySelector(`#record-form-container`).style.display = `block`;
    // запоминаем текущие данные формы
    initialData = getFormData(document.querySelector(`#record-form`));
}

// функция получения данных формы
function getFormData(form) {
    const formData = new FormData(form);
    const data = {};
    formData.forEach((value, key) => {
        data[key] = value;
    });
    data[`taken`] = document.querySelector(`#taken`).checked;
    
    return data;
}

// функция закрытия формы
function closeForm() {
    // получаем новые данные формы
    const currentData = getFormData(document.querySelector(`#record-form`));
    // проверка, были ли внесены изменения
    const hasChanges = Object.keys(initialData).some(key => initialData[key] !== currentData[key]);
    if (hasChanges == false || confirm(`Отменить внесенные изменения?`)) {
        clearForm();
        document.querySelector(`#record-form-container`).style.display = `none`;
    }
}

// функция очистки формы
function clearForm() {
    const form = document.querySelector(`#record-form`);
    Array.from(form.elements).forEach((element) => {
        if (element.type == `checkbox`) {
            element.checked = false;
        }
        else if (element.type == `select-one`) {
            element.selectedIndex = 0;
        }
        else {
            element.value = ``;
        }
    });
    document.querySelector(`#start`).value = new Date().toISOString().substring(0, 10);
}

// кнопка добавления записи
document.querySelector(`#add-record`).addEventListener(`click`, () => {
    openForm(0);
});

// кнопка закрытия формы
document.querySelector(`#close-record-form`).addEventListener(`click`, () => {
    closeForm();
});

// кнопка сохранения данных
document.querySelector(`#record-form`).addEventListener(`submit`, async function (event) {
    event.preventDefault();

    const recordData = getFormData(this);
    const method = document.querySelector(`#save-record-method`).value;

    // отправляем данные на сервер
    const response = await fetch(`/api/records/`, {
        method: method,
        headers: {
            "Content-Type": `application/json`
        },
        body: JSON.stringify(recordData)
    });
    if (response.ok) {
        clearForm();
        document.querySelector(`#record-form-container`).style.display = `none`;
        createTable();
    }
    else {
        alert(`Не удалось сохранить данные`);
    }
});

document.addEventListener('DOMContentLoaded', async function () {
    // заполняем выпадающий список сотрудниками
    await fillEmployees();
    // ставим сегодняшнюю дату для удобства
    document.querySelector(`#start`).value = new Date().toISOString().substring(0, 10);
});