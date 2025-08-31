

document.addEventListener("DOMContentLoaded", function () {
    const scrollButton = document.getElementById("scrollToElementBtn");
    const element = document.getElementsByClassName("section best-deal")[0]; // Get the first match

    scrollButton.addEventListener("click", function () {
      const bodyRect = document.body.getBoundingClientRect();
      const elemRect = element.getBoundingClientRect();
      const offset = elemRect.top - bodyRect.top;

      window.scrollTo({
        top: offset -70, // Adjust offset (80 = for header space etc.)
        behavior: 'smooth'
      });

      // Optional alert or log
      console.log('Scrolling to:', offset);
    });
  });