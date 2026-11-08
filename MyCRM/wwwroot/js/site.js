(() => {
  const toggle = document.querySelector('.taskbar__toggle');
  if (!toggle) return;

  const applyState = (collapsed) => {
    document.body.classList.toggle('sidebar-collapsed', collapsed);
    toggle.setAttribute('aria-expanded', String(!collapsed));
    const label = collapsed ? 'Rozwiń pasek boczny' : 'Zwiń pasek boczny';
    toggle.setAttribute('aria-label', label);
    toggle.title = label;
  };

  try {
    applyState(localStorage.getItem('mycrm.sidebar.collapsed') === 'true');
  } catch {
    applyState(false);
  }
  toggle.hidden = false;
  toggle.addEventListener('click', () => {
    const collapsed = !document.body.classList.contains('sidebar-collapsed');
    applyState(collapsed);
    try {
      localStorage.setItem('mycrm.sidebar.collapsed', String(collapsed));
    } catch {
      // The sidebar remains usable when browser storage is unavailable.
    }
  });
})();
