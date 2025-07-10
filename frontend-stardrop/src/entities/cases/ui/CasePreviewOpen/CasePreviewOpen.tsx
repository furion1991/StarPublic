import Image from 'next/image'
import { motion, AnimatePresence } from 'motion/react'
import cn from 'classnames'

import classes from './CasePreviewOpen.module.scss'

type CasePreviewOpenProps = {
  caseName: string
  previewImage: string
  previewsNumber: number
  imageType: 'FirstCategory' | 'SecondCategory'
}

export const CasePreviewOpen = ({
  caseName,
  previewImage,
  previewsNumber,
  imageType
}: CasePreviewOpenProps) => {
  return (
    <div className={classes.casePreviewOpen}>
      <AnimatePresence mode='popLayout'>
        {Array.from({ length: previewsNumber })
          .fill(null)
          .map((_, idx) => (
            <motion.div
              key={idx}
              layout
              initial={{
                opacity: idx === previewsNumber - 1 ? 0 : 1,
                x: 0
              }}
              animate={{
                x: 0,
                opacity: 1
              }}
              transition={{ ease: 'easeOut', duration: 0.15 }}
              className={cn(classes.caseImage, {
                [classes.withBoxBg]: previewsNumber !== 1,
                [classes.single]: previewsNumber === 1,
                [classes.three]: previewsNumber === 3,
                [classes.four]: previewsNumber === 4,
                [classes.five]: previewsNumber === 5,
                [classes.firstFormatImage]: imageType === 'FirstCategory',
                [classes.secondFormatImage]: imageType === 'SecondCategory'
              })}
            >
              <motion.div
                className={classes.imageContainer}
                initial={{ y: 100 }}
                animate={{ y: 0 }}
                transition={{ ease: 'circOut', duration: 0.3 }}
              >
                <Image src={previewImage} fill alt={caseName} priority />
              </motion.div>
            </motion.div>
          ))}
      </AnimatePresence>
    </div>
  )
}
