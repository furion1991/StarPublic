'use client'

import { useEffect, useRef, type RefObject } from 'react'

type UseGlowingAnimationProps = {
  elementsRef: RefObject<HTMLElement | null>[]
  enabled: boolean
}

export const useGlowingAnimation = ({ elementsRef, enabled }: UseGlowingAnimationProps) => {
  const animationsRef = useRef<Animation[]>([])

  useEffect(() => {
    const newAnimations: Animation[] = []

    if (!enabled) {
      animationsRef.current.forEach((animation) => animation.cancel())
      animationsRef.current = []
      return
    }

    elementsRef.forEach(({ current: element }, idx) => {
      if (!element) return

      const animation = element.animate(
        [
          { opacity: 0, visibility: 'hidden' },
          { opacity: 1, visibility: 'visible' }
        ],
        {
          duration: 700,
          iterations: Infinity,
          easing: 'ease-out',
          direction: 'alternate',
          delay: 700 * idx
        }
      )

      newAnimations.push(animation)
    })

    animationsRef.current = newAnimations

    return () => {
      newAnimations.forEach((animation) => animation.cancel())
    }
  }, [enabled])

  useEffect(() => {
    if (!enabled) return

    const speedUpIntervalId = setInterval(() => {
      animationsRef.current.forEach((animation) => {
        animation.updatePlaybackRate(animation.playbackRate + 1)
      })
    }, 1000)

    return () => {
      clearInterval(speedUpIntervalId)
    }
  }, [enabled])
}
