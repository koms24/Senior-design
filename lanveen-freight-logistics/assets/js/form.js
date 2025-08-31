let currentStep = 0;
const steps = document.querySelectorAll(".form-step");
const progressBar = document.getElementById("progress-bar");
const form = document.getElementById("multiStepForm");

// Load saved values from localStorage
steps.forEach((step, index) => {
  step.querySelectorAll("input").forEach(input => {
    const saved = localStorage.getItem(input.name);
    if (saved) input.value = saved;
  });
});

function showStep(index) {
  steps.forEach((step, i) => {
    step.classList.toggle("active", i === index);
  });
  progressBar.style.width = ((index + 1) / steps.length) * 100 + "%";
}

function nextStep() {
  saveInputs(currentStep);
  if (currentStep < steps.length - 1) {
    currentStep++;
    showStep(currentStep);
  }
}

function prevStep() {
  if (currentStep > 0) {
    currentStep--;
    showStep(currentStep);
  }
}

function saveInputs(stepIndex) {
  const inputs = steps[stepIndex].querySelectorAll("input");
  inputs.forEach(input => {
    localStorage.setItem(input.name, input.value);
  });
}

form.addEventListener("submit", function (e) {
  e.preventDefault();
  saveInputs(currentStep);

  const formData = {};
  form.querySelectorAll("input").forEach(input => {
    formData[input.name] = input.value;
  });

  console.log("Form Data:", formData);
  alert("Form submitted! (Check console)");

  // Clear storage
  localStorage.clear();
});
