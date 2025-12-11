using Swashbuckle.AspNetCore.SwaggerUI;

namespace SwaggerHierarchySupport;

public static class SwaggerUIExtensions
{
    public static void AddHierarchySupport(this SwaggerUIOptions opt, string? uri = null)
    {
        if (uri is null) uri = "https://unpkg.com/swagger-ui-plugin-hierarchical-tags";

        opt.HeadContent += $@"
                <script src='{uri}'></script>
                <script>
                (function() {{
                    let initialized = false;
    
                    function tryInitialize() {{
                        if (initialized) return;

                        if (window.ui && window.ui.getConfigs && typeof HierarchicalTagsPlugin !== 'undefined') {{
                            initialized = true;

                            const newUi = SwaggerUIBundle({{
                                ...window.ui.getConfigs(),
                                dom_id: '#swagger-ui',
                                plugins: [
                                    SwaggerUIBundle.plugins.DownloadUrl,
                                    HierarchicalTagsPlugin
                                    ]
                                }});
                                window.ui = newUi;
                            }}
                        }}
    
                        tryInitialize();
    
                        const interval = setInterval(() => {{
                            tryInitialize();
                            if (initialized) clearInterval(interval);
                        }}, 1);
    
                        function checkFrame() {{
                            tryInitialize();
                            if (!initialized) requestAnimationFrame(checkFrame);
                        }}
                        requestAnimationFrame(checkFrame);
                    }})();
                </script>";
    }
}