
 document.addEventListener("DOMContentLoaded", function () {
const yearDropdown = document.getElementById('yearDropdown');
const currentYear = new Date().getFullYear();

for (let i = currentYear - 4; i <= currentYear + 3; i++) {
  const option = document.createElement('option');
  option.value = i;
  option.text = i;
  if (i === currentYear) {
    option.selected = true; // Select the current year
  }
  yearDropdown.add(option);
}
 });