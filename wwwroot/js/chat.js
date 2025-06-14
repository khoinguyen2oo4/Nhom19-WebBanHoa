const connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();

let isSupportUser = false;
let selectedCustomerConnId = null;
let selectedCustomerName = null;

const chats = {};
const unread = new Set();
let connectionMap = {}; // connectionId => name

connection.on("SetUserRole", role => {
    isSupportUser = role === "Employer";

    if (!isSupportUser) {
        document.getElementById("customerList").style.display = "none";
        selectedCustomerName = null;
    } else {
        document.getElementById("customerList").style.display = "block";

        // ✅ CHUYỂN phần khôi phục vào đây:
        const savedName = sessionStorage.getItem("selectedCustomerName");
        const savedId = sessionStorage.getItem("selectedCustomerConnId");
        if (savedName && savedId) {
            selectedCustomerName = savedName;
            selectedCustomerConnId = savedId;
            console.log("🧩 Đã khôi phục khách đã chọn:", savedName, "ID:", savedId);
            connection.invoke("SelectCustomer", savedId);
        }
    }
});


connection.on("ReceiveMessage", (sender, message) => {
    if (!chats[sender]) chats[sender] = [];
    chats[sender].push(`<strong>${sender}</strong>: ${message}`);

    if (!isSupportUser && !selectedCustomerName) {
        selectedCustomerName = sender;
    }

    if (sender !== selectedCustomerName) {
        unread.add(sender);
        showToast(`Tin nhắn mới từ ${sender}`);
        try { document.getElementById("pingSound").play(); } catch { }
    }

    renderMessages();
    renderCustomerList();
});

connection.on("UpdateCustomerList", customers => {
    console.log("🔥 Danh sách khách từ server:", customers);

    connectionMap = {};
    customers.forEach(c => {
        console.log("📌 Tên khách:", c.name);
        connectionMap[c.connectionId] = c.name;
        if (!chats[c.name]) chats[c.name] = [];
    });
    renderCustomerList();
});

connection.start().then(() => {
   

    document.getElementById("sendButton").onclick = () => {
        const msg = document.getElementById("messageInput").value.trim();
        if (!msg) return;

        if (isSupportUser) {
            if (!selectedCustomerConnId || !selectedCustomerName) {
                alert("⚠️ Bạn chưa chọn khách hàng hoặc khách đã ngắt kết nối.");
                return;
            }

            if (!chats[selectedCustomerName]) chats[selectedCustomerName] = [];
            chats[selectedCustomerName].push(`<strong>Bạn</strong>: ${msg}`);
            connection.invoke("SendMessageToCustomer", selectedCustomerConnId, msg);
        } else {
            const target = Object.keys(chats).find(n => n.startsWith("Nhân viên")) || "Support";
            if (!chats[target]) chats[target] = [];
            chats[target].push(`<strong>Bạn</strong>: ${msg}`);
            connection.invoke("SendMessage", msg);
        }

        document.getElementById("messageInput").value = "";
        renderMessages();
    };
});

function renderCustomerList() {
    const list = document.getElementById("customerList");
    list.innerHTML = "";

    Object.entries(connectionMap).forEach(([connId, name]) => {
        if (!name.startsWith("Khách hàng")) return;

        const div = document.createElement("div");
        div.textContent = name;
        div.className = "customer-item" + (unread.has(name) ? " unread" : "");

        div.onclick = () => {
            selectedCustomerName = name;
            selectedCustomerConnId = connId;

            // ✅ Lưu vào sessionStorage
            sessionStorage.setItem("selectedCustomerName", name);
            sessionStorage.setItem("selectedCustomerConnId", connId);

            console.log("✅ Admin đã chọn:", name, "với ID:", connId);

            if (!selectedCustomerConnId) {
                showToast("⚠️ Không tìm thấy connectionId của khách.");
                return;
            }

            connection.invoke("SelectCustomer", selectedCustomerConnId);
            unread.delete(name);
            renderMessages();
            renderCustomerList();
        };

        list.appendChild(div);
    });
}

function renderMessages() {
    const box = document.getElementById("chatContent");
    if (selectedCustomerName && chats[selectedCustomerName]) {
        box.innerHTML = chats[selectedCustomerName].join("<br>");
    } else {
        box.innerHTML = "<em>Chọn khách để bắt đầu trò chuyện</em>";
    }
    box.scrollTop = box.scrollHeight;
}

function showToast(message) {
    const toast = document.createElement("div");
    toast.textContent = message;
    document.getElementById("toastContainer").appendChild(toast);
    setTimeout(() => toast.style.opacity = 1, 50);
    setTimeout(() => {
        toast.style.opacity = 0;
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

document.getElementById("closeChatBtn").onclick = () => {
    document.getElementById("chatPanel").style.display = "none";
    document.getElementById("openChatBtn").style.display = "block";
};

document.getElementById("openChatBtn").onclick = () => {
    document.getElementById("chatPanel").style.display = "flex";
    document.getElementById("openChatBtn").style.display = "none";
};
