// Sidebar toggle
document.addEventListener('DOMContentLoaded', function () {
    const toggle = document.getElementById('sidebarToggle');
    const sidebar = document.getElementById('sidebar');
    const mainContent = document.querySelector('.main-content');

    if (toggle && sidebar) {
        toggle.addEventListener('click', () => {
            if (window.innerWidth <= 768) {
                sidebar.classList.toggle('mobile-open');
            } else {
                sidebar.classList.toggle('collapsed');
                if (mainContent) mainContent.classList.toggle('expanded');
            }
        });
    }

    // Auto-dismiss alerts after 5s
    document.querySelectorAll('.alert-dismissible').forEach(alert => {
        setTimeout(() => {
            const bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
            if (bsAlert) bsAlert.close();
        }, 5000);
    });
});

// Fare Calculator (used in Booking/Create)
function calculateFare() {
    const roomId = document.getElementById('RoomId')?.value;
    const checkIn = document.getElementById('CheckInDate')?.value;
    const checkOut = document.getElementById('CheckOutDate')?.value;

    if (!roomId || !checkIn || !checkOut || roomId === '0') return;

    fetch(`/Room/GetFare?roomId=${roomId}&checkIn=${checkIn}&checkOut=${checkOut}`)
        .then(r => r.json())
        .then(data => {
            const el = id => document.getElementById(id);
            if (el('fareNights')) el('fareNights').textContent = data.nights + ' night(s)';
            if (el('fareRoomFare')) el('fareRoomFare').textContent = '₹' + data.fare.toFixed(2);
            if (el('fareTax')) el('fareTax').textContent = '₹' + data.tax.toFixed(2);
            if (el('fareTotal')) el('fareTotal').textContent = '₹' + data.total.toFixed(2);
            if (el('fareSummary')) el('fareSummary').style.display = 'block';
        })
        .catch(() => { });
}

// Confirm delete dialogs
document.querySelectorAll('[data-confirm]').forEach(btn => {
    btn.addEventListener('click', e => {
        if (!confirm(btn.dataset.confirm || 'Are you sure?')) e.preventDefault();
    });
});
