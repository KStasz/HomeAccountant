function ShowAlert(alertId, type, message) {
    const alertPlaceholder = document.getElementById(alertId)

    const existingAlert = document.getElementById('alert');

    if (existingAlert !== null && existingAlert !== undefined) {
        existingAlert.parentElement.remove();
    }

    message = message.replace(/\n/g, "<br />");
    
    const wrapper = document.createElement('div')
    wrapper.innerHTML = [
        `<div class="alert alert-${type} alert-dismissible" role="alert" id="alert">`,
        `   <div>${message}</div>`,
        '   <button type="button" class="btn-close" onclick="this.parentElement.parentElement.remove()"></button>',
        '</div>'
    ].join('')

        alertPlaceholder.append(wrapper)
}