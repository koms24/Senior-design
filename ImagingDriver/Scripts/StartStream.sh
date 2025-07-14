#!/bin/bash
rpicam-vid \
	-t 0 \
	-n \
	--width 1536 \
	--height 864 \
	--framerate 24 \
	--codec yuv420 \
	--libav-format rawvideo \
	-o - | \
		ffmpeg \
			-y \
			-f rawvideo \
			-pix_fmt yuv420p \
			-s:v 1536x864 \
			-re \
			-fflags +genpts \
			-i - \
			-c:v libx264 \
			-g:v 24 \
			-keyint_min:v 24 \
			-sc_threshold:v 0 \
			-preset ultrafast \
			-tune zerolatency \
			-use_timeline 0 \
			-streaming 1 \
			-window_size 2 \
			-extra_window_size 1 \
			-frag_type every_frame \
			-flags +global_header \
			-ldash 1 \
			-utc_timing_url 'https://time.akamai.com?iso&amp;ms' -format_options 'movflags=cmaf' \
			-timeout 0.5 \
			-write_prft 1 \
			-target_latency '3.0' \
			-f hls \
			-hls_time 1 \
			-hls_list_size 3 \
			-hls_segment_type fmp4 \
			-hls_flags delete_segments  \
			live.m3u8

