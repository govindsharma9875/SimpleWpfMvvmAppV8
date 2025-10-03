// Site JavaScript for Simple WPF to MVC Migration

// Document ready
document.addEventListener('DOMContentLoaded', function() {
    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Initialize popovers
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });

    // Auto-hide alerts
    setTimeout(function() {
        var alerts = document.querySelectorAll('.alert');
        alerts.forEach(function(alert) {
            if (alert.classList.contains('alert-success') || alert.classList.contains('alert-info')) {
                var bsAlert = new bootstrap.Alert(alert);
                setTimeout(function() {
                    bsAlert.close();
                }, 5000);
            }
        });
    }, 1000);

    // Add fade-in animation to cards
    var cards = document.querySelectorAll('.card');
    cards.forEach(function(card, index) {
        card.style.animationDelay = (index * 0.1) + 's';
        card.classList.add('fade-in');
    });

    // Form validation enhancements
    var forms = document.querySelectorAll('.needs-validation');
    forms.forEach(function(form) {
        form.addEventListener('submit', function(event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add('was-validated');
        });
    });

    // File input enhancements
    var fileInputs = document.querySelectorAll('input[type="file"]');
    fileInputs.forEach(function(input) {
        input.addEventListener('change', function() {
            var fileName = this.files[0] ? this.files[0].name : 'No file chosen';
            var label = this.nextElementSibling || this.parentElement.querySelector('.form-file-text');
            if (label) {
                label.textContent = fileName;
            }
        });
    });
});

// Global utility functions
window.SiteUtils = {
    // Show loading state
    showLoading: function(element) {
        if (element) {
            element.classList.add('loading');
            element.disabled = true;
        }
    },

    // Hide loading state
    hideLoading: function(element) {
        if (element) {
            element.classList.remove('loading');
            element.disabled = false;
        }
    },

    // Show toast notification
    showToast: function(message, type = 'info') {
        var toastContainer = document.getElementById('toast-container');
        if (!toastContainer) {
            toastContainer = document.createElement('div');
            toastContainer.id = 'toast-container';
            toastContainer.className = 'toast-container position-fixed top-0 end-0 p-3';
            toastContainer.style.zIndex = '1055';
            document.body.appendChild(toastContainer);
        }

        var toastId = 'toast-' + Date.now();
        var toastHtml = `
            <div id="${toastId}" class="toast" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="toast-header bg-${type} text-white">
                    <i class="fas fa-info-circle me-2"></i>
                    <strong class="me-auto">Notification</strong>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
                <div class="toast-body">
                    ${message}
                </div>
            </div>
        `;

        toastContainer.insertAdjacentHTML('beforeend', toastHtml);
        var toastElement = document.getElementById(toastId);
        var toast = new bootstrap.Toast(toastElement);
        toast.show();

        // Remove toast element after it's hidden
        toastElement.addEventListener('hidden.bs.toast', function() {
            toastElement.remove();
        });
    },

    // Confirm action with custom modal
    confirmAction: function(message, callback) {
        var confirmed = confirm(message);
        if (confirmed && typeof callback === 'function') {
            callback();
        }
        return confirmed;
    },

    // Format currency
    formatCurrency: function(amount) {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD'
        }).format(amount);
    },

    // Format date
    formatDate: function(dateString) {
        var date = new Date(dateString);
        return date.toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'short',
            day: 'numeric'
        });
    },

    // Validate email
    isValidEmail: function(email) {
        var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    },

    // Debounce function
    debounce: function(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }
};

// AJAX helper functions
window.AjaxHelper = {
    // GET request
    get: function(url, callback, errorCallback) {
        fetch(url, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
        .then(response => response.json())
        .then(data => {
            if (typeof callback === 'function') {
                callback(data);
            }
        })
        .catch(error => {
            if (typeof errorCallback === 'function') {
                errorCallback(error);
            } else {
                console.error('AJAX Error:', error);
                SiteUtils.showToast('An error occurred while processing your request.', 'danger');
            }
        });
    },

    // POST request
    post: function(url, data, callback, errorCallback) {
        var token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        
        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': token
            },
            body: JSON.stringify(data)
        })
        .then(response => response.json())
        .then(data => {
            if (typeof callback === 'function') {
                callback(data);
            }
        })
        .catch(error => {
            if (typeof errorCallback === 'function') {
                errorCallback(error);
            } else {
                console.error('AJAX Error:', error);
                SiteUtils.showToast('An error occurred while processing your request.', 'danger');
            }
        });
    }
};

// Search functionality
window.SearchHelper = {
    // Simple table search
    searchTable: function(searchInput, tableSelector) {
        var input = document.getElementById(searchInput);
        var table = document.querySelector(tableSelector);
        
        if (!input || !table) return;

        input.addEventListener('keyup', SiteUtils.debounce(function() {
            var filter = input.value.toUpperCase();
            var rows = table.getElementsByTagName('tr');

            for (var i = 1; i < rows.length; i++) {
                var row = rows[i];
                var cells = row.getElementsByTagName('td');
                var found = false;

                for (var j = 0; j < cells.length; j++) {
                    var cell = cells[j];
                    if (cell && cell.textContent.toUpperCase().indexOf(filter) > -1) {
                        found = true;
                        break;
                    }
                }

                row.style.display = found ? '' : 'none';
            }
        }, 300));
    }
};
