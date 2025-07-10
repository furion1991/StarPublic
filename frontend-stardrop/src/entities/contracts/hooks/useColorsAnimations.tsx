'use client'

import { useEffect, useRef, type RefObject } from 'react'

type UseColorsAnimationProps = {
  elementRef: RefObject<HTMLDivElement | null>
  enabled: boolean
}

export const useColorsAnimation = ({ elementRef, enabled }: UseColorsAnimationProps) => {
  const animationRef = useRef<Animation>(null)

  const colors = [
    'linear-gradient(180deg, #E171E3 0%, #8332ED 100%)',
    'linear-gradient(180deg, #3A7A18 0%, #68B21E 100%)',
    'linear-gradient(180deg, #66154F 0%, #D12A52 100%)',
    'linear-gradient(180deg, #B78A18 0%, #E5D11C 100%)'
  ]

  useEffect(() => {
    if (!elementRef.current) return

    if (!enabled) {
      animationRef.current?.cancel()
      animationRef.current = null
      return
    }

    const animation = elementRef.current.animate(
      colors.map((color) => ({ background: color })),
      {
        duration: 2000,
        iterations: Infinity,
        easing: 'linear'
      }
    )

    animationRef.current = animation
  }, [enabled])

  useEffect(() => {
    const animation = animationRef.current

    if (!enabled || !animation) return

    const speedUpIntervalId = setInterval(() => {
      animation.updatePlaybackRate(animation.playbackRate + 1)
    }, 1000)

    return () => {
      clearInterval(speedUpIntervalId)
    }
  }, [enabled])
}
