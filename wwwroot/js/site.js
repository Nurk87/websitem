const API_URL = "/api";
let currentUserId = null;
let myTasks = [];
let timerInterval = null;

// --- EKRAN GEÇİŞLERİ ---
function showLogin() {
    hideAll();
    document.getElementById('login-container').classList.remove('hidden');
}
function showRegister() {
    hideAll();
    document.getElementById('register-container').classList.remove('hidden');
}
function showForgot() {
    hideAll();
    document.getElementById('forgot-container').classList.remove('hidden');
}
function hideAll() {
    document.getElementById('login-container').classList.add('hidden');
    document.getElementById('register-container').classList.add('hidden');
    document.getElementById('forgot-container').classList.add('hidden');
    document.getElementById('todo-container').classList.add('hidden');
}

// --- KAYIT OL ---
async function register() {
    const user = document.getElementById('regUsername').value;
    const pass = document.getElementById('regPassword').value;
    const question = document.getElementById('regQuestion').value;
    const answer = document.getElementById('regAnswer').value;

    if (!user || !pass || !answer) return alert("Lütfen tüm alanları doldur!");

    try {
        const response = await fetch(`${API_URL}/Auth/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username: user, password: pass, securityQuestion: question, securityAnswer: answer })
        });

        if (response.ok) {
            alert("Kayıt Başarılı! Şimdi giriş yapabilirsin.");
            showLogin();
        } else {
            alert("Kayıt başarısız! Kullanıcı adı alınmış olabilir.");
        }
    } catch (err) { alert("Hata oluştu."); }
}

// --- GİRİŞ YAP ---
async function login() {
    const user = document.getElementById('loginUsername').value;
    const pass = document.getElementById('loginPassword').value;

    if (!user || !pass) return alert("Bilgileri girin!");

    try {
        const response = await fetch(`${API_URL}/Auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username: user, password: pass })
        });

        if (response.ok) {
            const data = await response.json();
            currentUserId = data.userId;
            hideAll();
            document.getElementById('todo-container').classList.remove('hidden');
            loadTasks();
            startClockCheck();
        } else {
            alert("Kullanıcı adı veya şifre yanlış!");
        }
    } catch (err) { alert("Bağlantı hatası."); }
}

// --- ŞİFRE SIFIRLAMA ---
async function resetPassword() {
    const user = document.getElementById('forgotUsername').value;
    const question = document.getElementById('forgotQuestion').value;
    const answer = document.getElementById('forgotAnswer').value;
    const newPass = document.getElementById('newPassword').value;

    if (!user || !answer || !newPass) return alert("Tüm alanları doldur!");

    try {
        const response = await fetch(`${API_URL}/Auth/reset-password`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                username: user,
                securityQuestion: question,
                securityAnswer: answer,
                newPassword: newPass
            })
        });

        if (response.ok) {
            alert("Şifren başarıyla değiştirildi! Giriş yapabilirsin.");
            showLogin();
        } else {
            alert("Bilgiler uyuşmuyor (Soru veya cevap yanlış).");
        }
    } catch (err) { alert("Hata oluştu."); }
}

// --- ÇIKIŞ YAP ---
function logout() {
    currentUserId = null;
    showLogin();
}

// --- GÖREV FONKSİYONLARI (Aynı kaldı) ---
async function loadTasks() {
    if (!currentUserId) return;
    const response = await fetch(`${API_URL}/Todo/${currentUserId}`);
    if (response.ok) {
        myTasks = await response.json();
        renderAll();
    }
}

function renderAll() {
    const activeList = document.getElementById('taskList');
    const historyList = document.getElementById('historyList');
    activeList.innerHTML = ""; historyList.innerHTML = "";

    myTasks.forEach(task => {
        let timePart = "--:--", textPart = task.title;
        if (task.title.includes(" - ")) {
            const parts = task.title.split(" - ");
            timePart = parts[0]; textPart = parts.slice(1).join(" - ");
        }
        if (task.isCompleted) {
            historyList.innerHTML += `<tr class="completed-row"><td>${timePart}</td><td>${textPart}</td><td>✔</td></tr>`;
        } else {
            activeList.innerHTML += `<tr><td><strong>${timePart}</strong></td><td>${textPart}</td><td><button onclick="markAsDone(${task.id})" class="check-btn">✔</button><button onclick="deleteTask(${task.id})" class="delete-btn">×</button></td></tr>`;
        }
    });
}

async function addTask() {
    const time = document.getElementById('timeInput').value;
    const text = document.getElementById('taskInput').value;
    if (!text) return alert("Görev yaz!");
    const fullText = time ? `${time} - ${text}` : text;
    await fetch(`${API_URL}/Todo`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ userId: currentUserId, title: fullText, isCompleted: false, orderIndex: 0 })
    });
    document.getElementById('taskInput').value = "";
    loadTasks();
}

async function markAsDone(id) { await fetch(`${API_URL}/Todo/toggle/${id}`, { method: 'PUT' }); loadTasks(); }
async function deleteTask(id) { if (confirm("Silinecek?")) { await fetch(`${API_URL}/Todo/${id}`, { method: 'DELETE' }); loadTasks(); } }

function startClockCheck() {
    if (timerInterval) clearInterval(timerInterval);
    timerInterval = setInterval(() => {
        const now = new Date();
        const currentTime = `${String(now.getHours()).padStart(2, '0')}:${String(now.getMinutes()).padStart(2, '0')}`;
        myTasks.forEach(task => {
            if (!task.isCompleted && task.title.startsWith(currentTime)) {
                let text = task.title.split(" - ")[1] || task.title;
                if (confirm(`⏰ SAAT GELDİ!\n${text}\nYaptın mı?`)) markAsDone(task.id);
            }
        });
    }, 15000);
}

function showActiveTasks() {
    document.getElementById('active-list-area').classList.remove('hidden');
    document.getElementById('history-list-area').classList.add('hidden');
    document.querySelectorAll('.nav-btn')[0].classList.add('active');
    document.querySelectorAll('.nav-btn')[1].classList.remove('active');
}
function showHistory() {
    document.getElementById('active-list-area').classList.add('hidden');
    document.getElementById('history-list-area').classList.remove('hidden');
    document.querySelectorAll('.nav-btn')[0].classList.remove('active');
    document.querySelectorAll('.nav-btn')[1].classList.add('active');
}