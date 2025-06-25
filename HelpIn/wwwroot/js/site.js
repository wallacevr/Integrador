// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
<script src="https://cdnjs.cloudflare.com/ajax/libs/countup.js/2.0.8/countUp.min.js"></script>

    document.addEventListener("DOMContentLoaded", () => {
        const options = { duration: 2, separator: '.' };

        const counters = [
            { id: 'counter-ongs', endVal: 200 },
            { id: 'counter-voluntarios', endVal: 800 },
            { id: 'counter-vagas', endVal: 1200 }
        ];

        counters.forEach(counter => {
            const el = document.getElementById(counter.id);
            if (el) {
                const observer = new IntersectionObserver(entries => {
                    entries.forEach(entry => {
                        if (entry.isIntersecting) {
                            const countUp = new CountUp(counter.id, counter.endVal, options);
                            countUp.start();
                            observer.unobserve(entry.target);
                        }
                    });
                });
                observer.observe(el);
            }
        });
    });


