import { openForm } from './recordForm.js';

// функция создания таблицы - списка записей
export async function createTable() {
    const recordsTable = document.querySelector(`#records-table tbody`);
    recordsTable.innerHTML = ``;

    // получаем данные о записях
    const response = await fetch(`/api/records/`, {
        method: `GET`,
        headers: {
            "Accept": `application/json`
        }
    });
    if (response.ok) {
        const recordsDataList = await response.json();

        if (recordsDataList.length > 0) {
            recordsDataList.forEach(recordData => {
                const row = document.createElement(`tr`);

                for (const key in recordData) {
                    if (recordData.hasOwnProperty(key)) {
                        const cell = document.createElement(`td`);
                        if (key === `taken`) {
                            cell.textContent = recordData[key] ? `Да` : `Нет`;
                        }
                        else {
                            cell.textContent = recordData[key];
                        }

                        row.append(cell);
                    }
                }

                const editCell = document.createElement(`td`);

                // кнопка изменения записи
                const editBtn = document.createElement(`span`);
                editBtn.className = `edit`;
                editBtn.id = `${recordData.id}`;
                editBtn.style.cursor = `pointer`;
                editBtn.innerHTML = `&#9998;`;
                editBtn.addEventListener('click', async function () {
                    await editRecord(editBtn.id);
                });

                editCell.append(editBtn);
                row.append(editCell);

                const deleteCell = document.createElement(`td`);

                // кнопка удаления записи
                const deleteBtn = document.createElement(`span`);
                deleteBtn.className = `delete`;
                deleteBtn.id = `${recordData.id}`;
                deleteBtn.style.cursor = `pointer`;
                deleteBtn.float = `right`;
                deleteBtn.innerHTML = `&times`;
                deleteBtn.addEventListener('click', async function () {
                    await deleteRecord(deleteBtn.id);
                });

                deleteCell.append(deleteBtn);
                row.append(deleteCell);

                recordsTable.append(row);
            });
        }
        else {
            const row = document.createElement(`tr`);
            const cell = document.createElement(`td`);
            cell.colSpan = 9;
            cell.innerHTML = `Нет данных`;
            row.append(cell);
            recordsTable.append(row);
        }
    }
    else {
        alert(`Не удалось получить данные о записях`);
    }
}

// кнопка изменения записи по id
async function editRecord(id) {
    openForm(id);
}

// кнопка удаления записи по id
async function deleteRecord(id) {
    // отправляем запрос на удаление
    const response = await fetch(`/api/records/${id}`, {
        method: `DELETE`,
        headers: {
            "Content-Type": `application/json`
        }
    });
    if (response.ok) {
        createTable();
    }
    else {
        alert(`Не удалось удалить`);
    }
}

document.addEventListener('DOMContentLoaded', async function () {
    await createTable();
});