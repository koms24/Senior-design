
  document.addEventListener("DOMContentLoaded", function () {
    const steps = document.querySelectorAll(".formbold-form-step-1, .formbold-form-step-2, .formbold-form-step-3");
    const menuItems = document.querySelectorAll(".formbold-steps li");
    const nextBtn = document.querySelector(".formbold-btn");
    const backBtn = document.querySelector(".formbold-back-btn");
    const form = document.querySelector("form");

    let currentStep = 0;

    function updateStepView() {
      steps.forEach((step, index) => {
        step.classList.toggle("active", index === currentStep);
      });

      menuItems.forEach((item, index) => {
        item.classList.toggle("active", index === currentStep);
      });

      backBtn.style.display = currentStep === 0 ? "none" : "inline-block";
      nextBtn.textContent = currentStep === steps.length - 1 ? "Submit" : "Next Step";
    }

    nextBtn.addEventListener("click", () => {
      if (currentStep < steps.length - 1) {
        currentStep++;
        updateStepView();
      } else {
        form.submit(); // submit only on final step
      }
    });

    backBtn.addEventListener("click", () => {
      if (currentStep > 0) {
        currentStep--;
        updateStepView();
      }
    });

    updateStepView(); // Initialize view
  });

