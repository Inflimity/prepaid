const API_URL = 'http://localhost:5193/api/users';
const API_KEY = 'super_secure_prepaid_api_key_2026';

// Elements
const usersTbody = document.getElementById('users-tbody');
const loadingIndicator = document.getElementById('loading-indicator');
const toastContainer = document.getElementById('toast-container');
const modalOverlay = document.getElementById('modal-overlay');

// Modals
const addUserModal = document.getElementById('add-user-modal');
const balanceActionModal = document.getElementById('balance-action-modal');

// Forms
const addUserForm = document.getElementById('add-user-form');
const balanceActionForm = document.getElementById('balance-action-form');

// Utilities
const formatNaira = (kobo) => {
  const naira = kobo / 100;
  return new Intl.NumberFormat('en-NG', { style: 'currency', currency: 'NGN' }).format(naira);
};

const formatDate = (dateString) => {
  const date = new Date(dateString);
  return date.toLocaleString();
};

const showToast = (message, type = 'success') => {
  const toast = document.createElement('div');
  toast.className = `toast ${type}`;
  toast.textContent = message;
  toastContainer.appendChild(toast);
  setTimeout(() => {
    toast.remove();
  }, 3000);
};

const apiFetch = async (url, options = {}) => {
  try {
    const response = await fetch(url, {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        'X-Api-Key': API_KEY,
        ...options.headers,
      },
    });
    
    if (response.status === 429) {
      throw new Error('Rate limit exceeded. Please wait a moment.');
    }

    const data = await response.json();
    
    if (!response.ok) {
      throw new Error(data.message || 'An error occurred');
    }
    
    return data;
  } catch (error) {
    showToast(error.message, 'error');
    throw error;
  }
};

// Fetch and Render Users
const loadUsers = async () => {
  try {
    loadingIndicator.style.display = 'block';
    usersTbody.innerHTML = '';
    
    const response = await apiFetch(API_URL);
    const users = response.data;
    
    if (users.length === 0) {
      usersTbody.innerHTML = '<tr><td colspan="6" class="text-center">No users found. Add one to get started.</td></tr>';
      return;
    }

    users.forEach(user => {
      const tr = document.createElement('tr');
      tr.innerHTML = `
        <td class="font-mono">#${user.id}</td>
        <td>${user.fullName}</td>
        <td>${user.phoneNumber}</td>
        <td class="font-mono">${formatNaira(user.balance)}</td>
        <td>${formatDate(user.lastUpdated)}</td>
        <td class="actions">
          <button class="btn btn-success btn-sm increase-btn" data-id="${user.id}" data-name="${user.fullName}">Increase</button>
          <button class="btn btn-danger btn-sm decrease-btn" data-id="${user.id}" data-name="${user.fullName}">Decrease</button>
        </td>
      `;
      usersTbody.appendChild(tr);
    });
  } catch (error) {
    usersTbody.innerHTML = '<tr><td colspan="6" class="text-center text-danger">Failed to load users.</td></tr>';
  } finally {
    loadingIndicator.style.display = 'none';
  }
};

// Modal Logic
const openModal = (modal) => {
  modalOverlay.classList.remove('hidden');
  modal.classList.remove('hidden');
};

const closeAllModals = () => {
  modalOverlay.classList.add('hidden');
  document.querySelectorAll('.modal').forEach(m => m.classList.add('hidden'));
  addUserForm.reset();
  balanceActionForm.reset();
};

document.getElementById('add-user-btn').addEventListener('click', () => {
  openModal(addUserModal);
});

document.querySelectorAll('.close-btn').forEach(btn => {
  btn.addEventListener('click', closeAllModals);
});

// Event Delegation for Table Buttons
usersTbody.addEventListener('click', (e) => {
  if (e.target.classList.contains('increase-btn') || e.target.classList.contains('decrease-btn')) {
    const id = e.target.dataset.id;
    const name = e.target.dataset.name;
    const isIncrease = e.target.classList.contains('increase-btn');
    
    document.getElementById('action-user-id').value = id;
    document.getElementById('action-type').value = isIncrease ? 'increase' : 'decrease';
    document.getElementById('balance-modal-title').textContent = isIncrease ? 'Increase Balance' : 'Decrease Balance';
    document.getElementById('action-user-name').textContent = `Target: ${name}`;
    
    const submitBtn = document.getElementById('balance-submit-btn');
    submitBtn.className = `btn ${isIncrease ? 'btn-success' : 'btn-danger'}`;
    
    openModal(balanceActionModal);
  }
});

// Add User Form Submit
addUserForm.addEventListener('submit', async (e) => {
  e.preventDefault();
  
  const fullName = document.getElementById('fullname').value;
  const phoneNumber = document.getElementById('phone').value;
  const initialBalanceNGN = parseFloat(document.getElementById('initial-balance').value);
  const balanceKobo = Math.round(initialBalanceNGN * 100);

  try {
    await apiFetch(API_URL, {
      method: 'POST',
      body: JSON.stringify({ fullName, phoneNumber: parseInt(phoneNumber), balance: balanceKobo })
    });
    
    showToast('User created successfully');
    closeAllModals();
    loadUsers();
  } catch (error) {
    // Handled in apiFetch
  }
});

// Balance Action Form Submit
balanceActionForm.addEventListener('submit', async (e) => {
  e.preventDefault();
  
  const id = document.getElementById('action-user-id').value;
  const type = document.getElementById('action-type').value;
  const amountNGN = parseFloat(document.getElementById('action-amount').value);
  const amountKobo = Math.round(amountNGN * 100);

  try {
    await apiFetch(`${API_URL}/${id}/${type}`, {
      method: 'PATCH',
      body: JSON.stringify({ amount: amountKobo })
    });
    
    showToast(`Balance ${type}d successfully`);
    closeAllModals();
    loadUsers();
  } catch (error) {
    // Handled in apiFetch
  }
});

// Init
loadUsers();
